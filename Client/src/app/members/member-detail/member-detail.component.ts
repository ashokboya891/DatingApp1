import { Component, OnInit } from '@angular/core';
import { ActivatedRoute } from '@angular/router';
import { Member } from 'src/app/_models/member';
import { MembersService } from 'src/app/_services/members.service';
import {  CommonModule} from "@angular/common";
import { TabsModule } from 'ngx-bootstrap/tabs';
import { GalleryItem, GalleryModule, ImageItem } from 'ng-gallery';
import { NgxGalleryAnimation, NgxGalleryImage, NgxGalleryOptions } from '@kolkov/ngx-gallery';
import { Gallery } from 'ng-gallery';
import { TimeagoModule } from 'ngx-timeago';

@Component({
  selector: 'app-member-detail',
  standalone:true,
  templateUrl: './member-detail.component.html',
  styleUrls: ['./member-detail.component.css'],
  imports: [CommonModule,TabsModule,GalleryModule,TimeagoModule]
})

export class MemberDetailComponent  implements OnInit{
  images:GalleryItem[]=[];
member:Member|undefined;

  ngOnInit(): void {
    this.loadMember();
   
     
  }
  constructor(private memberService:MembersService,private route:ActivatedRoute) {
    
  }
  loadMember()
  {
    const username=this.route.snapshot.paramMap.get('username');
    if(!username)return;
    this.memberService.getMember(username).subscribe({
      next:member=>{this.member=member
      this.getImages();
      }
    })
  }
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
