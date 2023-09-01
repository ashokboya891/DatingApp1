import { Component, OnInit, ViewChild ,OnDestroy} from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Member } from 'src/app/_models/member';
import { MembersService } from 'src/app/_services/members.service';
import {  CommonModule} from "@angular/common";
import { TabDirective, TabsModule, TabsetComponent } from 'ngx-bootstrap/tabs';
import { GalleryItem, GalleryModule, ImageItem } from 'ng-gallery';
import { NgxGalleryAnimation, NgxGalleryImage, NgxGalleryOptions } from '@kolkov/ngx-gallery';
import { Gallery } from 'ng-gallery';
import { TimeagoModule } from 'ngx-timeago';
import { MemberMessagesComponent } from '../member-messages/member-messages.component';
import { MessageService } from 'src/app/_services/message.service';
import { Message } from 'src/app/_models/message';
import { PresenceService } from 'src/app/_services/presence.service';
import { AccoutnService } from 'src/app/_services/accoutn.service';
import { User } from 'src/app/_models/User';
import { take } from 'rxjs';

@Component({
  selector: 'app-member-detail',
  standalone:true,
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css'],
  imports: [CommonModule,TabsModule,GalleryModule,TimeagoModule,MemberMessagesComponent]
})

export class MemberDetailComponent  implements OnInit,OnDestroy{
  @ViewChild('memberTabs',{static:true})memberTabs?:TabsetComponent;
  images:GalleryItem[]=[];
member:Member={} as Member;
messages:Message[]=[];
activeTab?:TabDirective;
user?:User;

  ngOnInit(): void {
    this.route.data.subscribe({
      next:data=>this.member=data['member']
    })
    // this.loadMember();
    this.route.queryParams.subscribe({
      next:params=>{
        params['tab'] &&  this.selectTab(params['tab'])
      }
    })
    this.getImages();  
  }
  ngOnDestroy(): void {
    this.messageService.stopHubConnection();
  }
  constructor(private memberService:MembersService,private route:ActivatedRoute,
  private messageService:MessageService,public presenceService:PresenceService,private accountService:AccoutnService) 
  {
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next:user=>{
        if(user)this.user=user;
      }
      
    })
  }

  selectTab(heading:string)
  {
    if(this.memberTabs)
    {
      this.memberTabs.tabs.find(x=>x.heading===heading)!.active=true;

    }
  }
  onTabActivated(data:TabDirective)
  {
    this.activeTab=data;
    if(this.activeTab.heading==='Messages' && this.user)
    {
      this.messageService.createHubConnection(this.user,this.member.userName);
      // this.loadMessages();
    }
    else{
      this.messageService.stopHubConnection();
    }
  }
  loadMessages()
  {
    if(this.member)
    {
      this.messageService.getMessageThread(this.member.userName).subscribe({
        next:messages=>{
          this.messages=messages;
        }
      })
    }
  }

  //this method replaced in ngonit with respect to member detailed resolver.ts
  // loadMember()
  // {
  //   const username=this.route.snapshot.paramMap.get('username');
  //   if(!username)return;
  //   this.memberService.getMember(username).subscribe({
  //     next:member=>{this.member=member
  //     this.getImages();
  //     }
  //   })
  // }
  getImages()
  {
    if(!this.member)return;
    // const imageUrls=[];
    
    for(const photo of this.member?.photos)
    {
        this.images.push(new ImageItem({src:photo.url,thumb:photo.url}))
        this.images.push(new ImageItem({src:photo.url,thumb:photo.url})),
        this.images.push(new ImageItem({src:photo.url,thumb:photo.url}))
    }
  }
}
