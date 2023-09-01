import { CommonModule } from '@angular/common';
import { ChangeDetectionStrategy } from '@angular/core';
import { Component, Input, OnInit, ViewChild } from '@angular/core';
import { FormsModule, NgForm } from '@angular/forms';
import { TimeagoModule } from 'ngx-timeago';
import { Message } from 'src/app/_models/message';
import { MessageService } from 'src/app/_services/message.service';

@Component({
  changeDetection: ChangeDetectionStrategy.OnPush,
  selector: 'app-member-messages',
  standalone:true,
  templateUrl: './member-messages.component.html',
  styleUrls: ['./member-messages.component.css'],
  imports:[CommonModule,TimeagoModule,FormsModule]
})
export class MemberMessagesComponent implements OnInit {
  @ViewChild('messageForm') messageForm?:NgForm;
  @Input() username?:string;
  // @Input() messages:Message[]=[];  after adding messages hub we are commenting this arrya message collection
  messageContent='';
  // messages:Message[]=[];
  ngOnInit(): void {
    // this.sendMessage();
      // this.loadMessages();
  }
  constructor(public messageService:MessageService) {
    
  }
  sendMessage()
  {
    if(!this.username) return;
    this.messageService.sendMessage(this.username,this.messageContent).then(()=>{
        this.messageForm.reset();
    })
      
        //after updagting code in sendmesage method in message service below lines commented
        // this.messages.push(message);
        // this.messageForm.reset();
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
