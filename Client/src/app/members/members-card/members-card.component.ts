import { Component, Input, OnInit, ViewEncapsulation } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { Member } from 'src/app/_models/member';
import { MembersService } from 'src/app/_services/members.service';
import { PresenceService } from 'src/app/_services/presence.service';

@Component({
  selector: 'app-members-card',
  templateUrl: './members-card.component.html',
  styleUrls: ['./members-card.component.css'],
 encapsulation:ViewEncapsulation.None
})
export class MembersCardComponent implements OnInit{
@Input () member:Member | undefined;
constructor(private memberService:MembersService,private toastr:ToastrService,public presenceService:PresenceService)
{

}
ngOnInit(): void {
    
}
addLike(member:Member)
{
  this.memberService.addLike(member.userName).subscribe({
    next:()=>this.toastr.success("you have liked "+member.knownAs)
  })
}

}
