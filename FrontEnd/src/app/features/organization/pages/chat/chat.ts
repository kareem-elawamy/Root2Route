import { Component, OnInit, OnDestroy, inject, ChangeDetectorRef, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ChatService } from '../../chat.service';
import { OrgContextService } from '../../../../core/services/org-context.service';
import { AuthService } from '../../../../core/services/auth.service';
import { FormsModule } from '@angular/forms';

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
  private cdr = inject(ChangeDetectorRef);
  private authService = inject(AuthService);

  get currentUserId() {
    return this.authService.currentUser()?.id;
  }

  rooms: any[] = [];
  activeRoom = signal<any>(null);
  messages: any[] = [];
  newMessage = '';
  isHelpOpen = signal(false);
  private pollingInterval: any;

  toggleChatHelp() {
    this.isHelpOpen.update(v => !v);
  }

  closeChatHelp() {
    this.isHelpOpen.set(false);
  }

  ngOnInit() {
    this.loadRooms();
    
    // Auto-refresh chat and rooms every 5 seconds
    this.pollingInterval = setInterval(() => {
      this.loadRooms();
      const room = this.activeRoom();
      if (room) {
        this.loadHistory(room.id, true);
      }
    }, 5000);
  }

  ngOnDestroy() {
    if (this.pollingInterval) {
      clearInterval(this.pollingInterval);
    }
  }

  loadRooms() {
    this.chatService.getMyRooms().subscribe({
      next: (response: any) => {
        const data = response.data || response || [];
        this.rooms = (Array.isArray(data) ? data : []).map((r: any) => ({
          ...r,
          otherParticipantName: r.otherPartyName,
          lastMessage: r.lastMessageSnippet
        }));
        this.cdr.detectChanges();
      },
      error: (error: any) => {
        console.error('Error fetching chat rooms', error);
      }
    });
  }

  selectRoom(room: any) {
    this.activeRoom.set(room);
    this.loadHistory(room.id);
    this.chatService.markAsRead(room.id).subscribe();
  }

  loadHistory(roomId: string, isPolling = false) {
    this.chatService.getHistory(roomId).subscribe({
      next: (response: any) => {
        const data = response.data || response || [];
        const userId = this.currentUserId;
        
        this.messages = (Array.isArray(data) ? data : []).map((msg: any) => {
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
        });
        this.cdr.detectChanges();
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
      alert('Sharing emails or phone numbers is not allowed for privacy reasons.');
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
        alert('Offer accepted!');
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
        alert('Offer rejected!');
        const room = this.activeRoom();
        if (room) this.loadHistory(room.id);
      },
      error: (error: any) => {
        console.error('Error rejecting offer', error);
      }
    });
  }
}
