import { Component, OnInit, inject, ChangeDetectorRef, signal } from '@angular/core';
import { CommonModule } from '@angular/common';
import { ChatService } from '../../chat.service';
import { OrgContextService } from '../../../../core/services/org-context.service';
import { FormsModule } from '@angular/forms';

@Component({
  selector: 'app-chat',
  standalone: true,
  imports: [CommonModule, FormsModule],
  templateUrl: './chat.html'
})
export class ChatComponent implements OnInit {
  private chatService = inject(ChatService);
  private orgCtx = inject(OrgContextService);
  private cdr = inject(ChangeDetectorRef);

  rooms: any[] = [];
  activeRoom = signal<any>(null);
  messages: any[] = [];
  newMessage = '';

  ngOnInit() {
    this.loadRooms();
  }

  loadRooms() {
    this.chatService.getMyRooms().subscribe({
      next: (response: any) => {
        const data = response.data || response || [];
        this.rooms = Array.isArray(data) ? data : [];
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

  loadHistory(roomId: string) {
    this.chatService.getHistory(roomId).subscribe({
      next: (response: any) => {
        const data = response.data || response || [];
        this.messages = Array.isArray(data) ? data : [];
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
