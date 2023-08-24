import { Component, HostListener, OnInit, ViewChild } from '@angular/core';
import { NgForm } from '@angular/forms';
import { ToastrService } from 'ngx-toastr';
import { take } from 'rxjs';
import { User } from 'src/app/_models/User';
import { Member } from 'src/app/_models/member';
import { AccoutnService } from 'src/app/_services/accoutn.service';
import { MembersService } from 'src/app/_services/members.service';

@Component({
  selector: 'app-member-edit',
  templateUrl: './member-edit.component.html',
  styleUrls: ['./member-edit.component.css']
})
export class MemberEditComponent implements OnInit {
  
@ViewChild('editForm') editForm:NgForm|undefined;
@HostListener('window:beforeunload',['$event'])unloadNotification($event:any)
{
  if(this.editForm?.dirty)
  {
    $event.returnValue=true;
  }
}
  member:Member|undefined;
  user:User|null=null;
  constructor(private accountService:AccoutnService,private memberService:MembersService,private taostr:ToastrService) 
  {
    this.accountService.currentUser$.pipe(take(1)).subscribe({
      next:user=>this.user=user
    })
    
  }
  ngOnInit(): void {
     this.loadMember(); 
  }
  loadMember()
  {
    if(!this.user)return;
    this.memberService.getMember(this.user.username).subscribe({
      next:member=>this.member=member
    })
  }
  updateMember()
  {
    this.memberService.updateMember(this.editForm?.value).subscribe({
      next:_=>{
        this.editForm.reset(this.member);
        this.taostr.success("profile updated");
      }
    })
   
   
    
  }
}
