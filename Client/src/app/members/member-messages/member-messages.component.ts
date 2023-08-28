import { CommonModule } from '@angular/common';
import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { TimeagoModule } from 'ngx-timeago';
import { Message } from 'src/app/_models/message';
import { MessageService } from 'src/app/_services/message.service';

@Component({
  selector: 'app-member-messages',
  standalone:true,
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css'],
  imports:[CommonModule,TimeagoModule,FormsModule]
})
export class MemberMessagesComponent implements OnInit {
  @ViewChild('messageForm') messageForm?:NgForm;
  @Input() username?:string;
  @Input() messages:Message[]=[];
  messageContent='';
  // messages:Message[]=[];
  ngOnInit(): void {
    // this.sendMessage();
      // this.loadMessages();
  }
  constructor(private messageService:MessageService) {
    
  }
  sendMessage()
  {
    if(!this.username) return;
    this.messageService.sendMessage(this.username,this.messageContent).subscribe({
      next:message=>{
        this.messages.push(message);
        this.messageForm.reset();
      }
    })
   

  }

  // loadMessages()
  // {
  //   if(this.username)
  //   {
  //     this.messageService.getMessageThread(this.username).subscribe({
  //       next:messages=>{
  //         this.messages=messages;
  //       }
  //     })
  //   }
  // }
}
