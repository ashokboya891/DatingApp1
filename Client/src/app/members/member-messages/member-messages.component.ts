import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy } from '@angular/core';
import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { TimeagoModule } from 'ngx-timeago';
import { Message } from 'src/app/_models/message';
import { MessageService } from 'src/app/_services/message.service';
import { PresenceService } from 'src/app/_services/presence.service';
import { ToastrService } from 'ngx-toastr';

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  selector: 'app-member-messages',
  standalone: true,
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css'],
  imports: [CommonModule, FormsModule, TimeagoModule]
})
export class MemberMessagesComponent implements OnInit {

  @ViewChild('messageForm') messageForm?: NgForm;
  @Input() username?: string;
  
  messageContent = '';
  loading = false;
  isTyping = '';
  typingTimer: any;

  constructor(
    public messageService: MessageService,
    private presenceService: PresenceService,
    private toastr: ToastrService
  ) {}

  ngOnInit(): void {
    // ✅ Listen for "User Is Typing" event
    this.presenceService.hubConnection.on('UserTyping', (username: string) => {

      // ✅ Prevent showing your own typing status
      if (username === this.username) return;

      // ✅ Show pop-up notification when someone is typing
      this.toastr.info(`${username} is typing...`, '', {
        timeOut: 2000, // Auto clear popup in 2 seconds
        disableTimeOut: false,
        progressBar: true
      });

      // ✅ Also show "is typing..." text below input box
      this.isTyping = `${username} is typing...`;

      // ✅ Clear the "is typing..." text after 2 seconds
      clearTimeout(this.typingTimer);
      this.typingTimer = setTimeout(() => {
        this.isTyping = '';
      }, 2000);
    });
  }

  // ✅ This method sends "User Is Typing..." status
  onTyping() {
    if (!this.username) return;
    
    // ✅ Send typing status to SignalR Hub
    this.presenceService.sendTypingStatus(this.username);

    // ✅ Clear typing status if the user is idle for 2 seconds
    clearTimeout(this.typingTimer);
    this.typingTimer = setTimeout(() => {
      this.isTyping = '';
    }, 2000);
  }

  // ✅ Send Message Function
  sendMessage() {
    if (!this.username) return;
    this.loading = true;

    this.messageService.sendMessage(this.username, this.messageContent)
      .then(() => {
        // ✅ Reset the form after sending the message
        this.messageForm?.reset();
        this.isTyping = '';  // ✅ Instantly clear typing status after sending message
      })
      .finally(() => this.loading = false);
  }
}
