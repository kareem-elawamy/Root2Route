import { ComponentFixture, TestBed } from '@angular/core/testing';
import { ChatComponent } from './chat';
import { ChatService } from '../../chat.service';
import { OrgContextService } from '../../../../core/services/org-context.service';
import { AuthService } from '../../../../core/services/auth.service';
import { ToastService } from '../../../../core/services/toast.service';
import { of } from 'rxjs';
import { FormsModule } from '@angular/forms';

describe('ChatComponent', () => {
  let component: ChatComponent;
  let fixture: ComponentFixture<ChatComponent>;
  let chatServiceSpy: any;
  let authServiceSpy: any;
  let toastSpy: any;

  beforeEach(async () => {
    chatServiceSpy = {
      getMyRooms: vi.fn().mockReturnValue(of({ data: [{ id: 'room1', otherPartyName: 'Bob' }] })),
      getHistory: vi.fn().mockReturnValue(of({ data: [{ id: 'msg1', content: 'Hello', senderId: 'user1' }] })),
      markAsRead: vi.fn().mockReturnValue(of({})),
      sendMessage: vi.fn().mockReturnValue(of({})),
      acceptOffer: vi.fn().mockReturnValue(of({})),
      rejectOffer: vi.fn().mockReturnValue(of({}))
    };

    authServiceSpy = {
      currentUser: vi.fn().mockReturnValue({ id: 'user1' })
    };

    toastSpy = {
      warning: vi.fn(),
      success: vi.fn()
    };

    const orgContextSpy = {};

    await TestBed.configureTestingModule({
      imports: [ChatComponent, FormsModule],
      providers: [
        { provide: ChatService, useValue: chatServiceSpy },
        { provide: AuthService, useValue: authServiceSpy },
        { provide: ToastService, useValue: toastSpy },
        { provide: OrgContextService, useValue: orgContextSpy }
      ]
    }).compileComponents();

    fixture = TestBed.createComponent(ChatComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });

  it('should load rooms on init', () => {
    expect(chatServiceSpy.getMyRooms).toHaveBeenCalled();
    expect(component.rooms().length).toBe(1);
    expect(component.rooms()[0].otherParticipantName).toBe('Bob');
  });

  it('should load history and mark as read when room is selected', () => {
    const room = { id: 'room1' };
    component.selectRoom(room);
    
    expect(component.activeRoom()).toEqual(room);
    expect(chatServiceSpy.getHistory).toHaveBeenCalledWith('room1');
    expect(chatServiceSpy.markAsRead).toHaveBeenCalledWith('room1');
    
    expect(component.messages().length).toBe(1);
    expect(component.messages()[0].content).toBe('Hello');
    expect(component.messages()[0].isMine).toBe(true); // Since senderId matches currentUserId
  });

  it('should prevent sending emails or phone numbers', () => {
    component.activeRoom.set({ id: 'room1' });
    
    component.newMessage = 'Call me at 123-456-7890';
    component.sendMessage();
    expect(toastSpy.warning).toHaveBeenCalled();
    expect(chatServiceSpy.sendMessage).not.toHaveBeenCalled();

    component.newMessage = 'Email me at test@example.com';
    component.sendMessage();
    expect(toastSpy.warning).toHaveBeenCalled();
    expect(chatServiceSpy.sendMessage).not.toHaveBeenCalled();
  });

  it('should send valid message', () => {
    component.activeRoom.set({ id: 'room1' });
    component.newMessage = 'Valid message';
    
    component.sendMessage();
    
    expect(chatServiceSpy.sendMessage).toHaveBeenCalled();
    expect(component.newMessage).toBe('');
    expect(chatServiceSpy.getHistory).toHaveBeenCalledTimes(1); // called inside sendMessage's subscribe
  });

  it('should poll for new messages only when room is selected', () => {
    vi.useFakeTimers();
    // Initial call from selectRoom
    component.selectRoom({ id: 'room1' });
    chatServiceSpy.getHistory.mockClear();
    
    // Advance time by 5 seconds
    vi.advanceTimersByTime(5000);
    
    // Should have polled
    expect(chatServiceSpy.getHistory).toHaveBeenCalledWith('room1');
    
    // Cleanup
    component.ngOnDestroy();
    vi.useRealTimers();
  });
});
