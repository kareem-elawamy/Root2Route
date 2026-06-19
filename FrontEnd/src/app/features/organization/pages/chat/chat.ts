import { Component, OnInit, OnDestroy, inject, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ChatService } from '../../chat.service';
import { OrgContextService } from '../../../../core/services/org-context.service';
import { AuthService } from '../../../../core/services/auth.service';
import { FormsModule } from '@angular/forms';
import { ToastService } from '../../../../core/services/toast.service';
import { Subscription, BehaviorSubject, timer, EMPTY } from 'rxjs';
import { switchMap } from 'rxjs/operators';

@Component({
  selector: 'app-chat',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './chat.html',
  // styleUrl: './chat.css'
})
export class ChatComponent implements OnInit, OnDestroy {
  private chatService = inject(ChatService);
  private orgCtx = inject(OrgContextService);
  private authService = inject(AuthService);
  private toast = inject(ToastService);

  get currentUserId() {
    return this.authService.currentUser()?.id;
  }

  rooms = signal<any[]>([]);
  activeRoom = signal<any>(null);
  messages = signal<any[]>([]);
  newMessage = '';
  isHelpOpen = signal(false);

  private activeRoom$ = new BehaviorSubject<any>(null);
  private pollingSub?: Subscription;

  toggleChatHelp() {
    this.isHelpOpen.update(v => !v);
  }

  closeChatHelp() {
    this.isHelpOpen.set(false);
  }

  ngOnInit() {
    this.loadRooms();

    // Smart polling: only poll when a room is selected
    this.pollingSub = this.activeRoom$.pipe(
      switchMap(room => room ? timer(5000, 5000) : EMPTY)
    ).subscribe(() => {
      this.loadRooms();
      const room = this.activeRoom();
      if (room) {
        this.loadHistory(room.id, true);
      }
    });
  }

  ngOnDestroy() {
    this.pollingSub?.unsubscribe();
    this.activeRoom$.complete();
  }

  loadRooms() {
    this.chatService.getMyRooms().subscribe({
      next: (response: any) => {
        const data = response.data || response || [];
        this.rooms.set((Array.isArray(data) ? data : []).map((r: any) => ({
          ...r,
          otherParticipantName: r.otherPartyName,
          lastMessage: r.lastMessageSnippet
        })));
      },
      error: (error: any) => {
        console.error('Error fetching chat rooms', error);
      }
    });
  }

  selectRoom(room: any) {
    this.activeRoom.set(room);
    this.activeRoom$.next(room); // trigger smart polling
    this.loadHistory(room.id);
    this.chatService.markAsRead(room.id).subscribe();
  }

  loadHistory(roomId: string, isPolling = false) {
    this.chatService.getHistory(roomId).subscribe({
      next: (response: any) => {
        const data = response.data || response || [];
        const userId = this.currentUserId;
        
        this.messages.set((Array.isArray(data) ? data : []).map((msg: any) => {
          let offerStatus = '';
          if (msg.type === 3) offerStatus = 'Pending';
          else if (msg.type === 4) offerStatus = 'Accepted';
          else if (msg.type === 5) offerStatus = 'Rejected';

          let createdAtStr = msg.sentAt;
          if (createdAtStr && !createdAtStr.endsWith('Z')) {
            createdAtStr += 'Z';
          }

          return {
            id: msg.id,
            isMine: msg.senderId === userId,
            content: msg.content,
            createdAt: createdAtStr,
            hasOffer: msg.type >= 3 && msg.type <= 5,
            offerAmount: msg.proposedPrice,
            offerDetails: msg.proposedQuantity ? `Quantity: ${msg.proposedQuantity}` : '',
            offerStatus: offerStatus,
            offerId: msg.id
          };
        }));
      },
      error: (error: any) => {
        console.error('Error fetching chat history', error);
      }
    });
  }

  sendMessage() {
    const room = this.activeRoom();
    if (!room || !this.newMessage.trim()) return;

    // Filter out emails and phone numbers for security/privacy
    const emailRegex = /[\w.-]+@[\w.-]+\.\w+/;
    const phoneRegex = /(?:\d[\s-]*){8,15}/;

    if (emailRegex.test(this.newMessage) || phoneRegex.test(this.newMessage)) {
      this.toast.warning('Sharing emails or phone numbers is not allowed for privacy reasons.');
      return;
    }

    const formData = new FormData();
    formData.append('ChatRoomId', room.id);
    formData.append('Content', this.newMessage);

    this.chatService.sendMessage(formData).subscribe({
      next: () => {
        this.newMessage = '';
        this.loadHistory(room.id);
      },
      error: (error: any) => {
        console.error('Error sending message', error);
      }
    });
  }

  acceptOffer(offerId: string) {
    this.chatService.acceptOffer({ offerId }).subscribe({
      next: () => {
        this.toast.success('Offer accepted!');
        const room = this.activeRoom();
        if (room) this.loadHistory(room.id);
      },
      error: (error: any) => {
        console.error('Error accepting offer', error);
      }
    });
  }

  rejectOffer(offerId: string) {
    this.chatService.rejectOffer({ offerId }).subscribe({
      next: () => {
        this.toast.warning('Offer rejected.');
        const room = this.activeRoom();
        if (room) this.loadHistory(room.id);
      },
      error: (error: any) => {
        console.error('Error rejecting offer', error);
      }
    });
  }

  closeChatRoom() {
    const room = this.activeRoom();
    if (!room) return;
    this.chatService.closeChat(room.id).subscribe({
      next: () => {
        this.toast.success('Chat room closed.');
        this.activeRoom.set(null);
        this.activeRoom$.next(null);
        this.messages.set([]);
        this.loadRooms();
      },
      error: (error: any) => {
        console.error('Error closing chat room', error);
        this.toast.error('Failed to close chat room.');
      }
    });
  }

  deleteMsg(messageId: string) {
    this.chatService.deleteMessage(messageId).subscribe({
      next: () => {
        this.toast.success('Message deleted.');
        const room = this.activeRoom();
        if (room) this.loadHistory(room.id);
      },
      error: (error: any) => {
        console.error('Error deleting message', error);
        this.toast.error('Failed to delete message.');
      }
    });
  }
}
