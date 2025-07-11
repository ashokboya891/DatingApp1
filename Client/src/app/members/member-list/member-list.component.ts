import { Component, OnInit } from '@angular/core';
import { Observable, take } from 'rxjs';
import { User } from 'src/app/_models/User';
import { Member } from 'src/app/_models/member';
import { Pagination } from 'src/app/_models/pagination';
import { userParams } from 'src/app/_models/userParams';
import { AccoutnService } from 'src/app/_services/accoutn.service';
import {MembersService  } from "src/app/_services/members.service";
@Component({
  selector: 'app-member-list',
  templateUrl: './member-list.component.html',
  styleUrls: ['./member-list.component.css']
})
export class MemberListComponent implements OnInit{

  
  members:Member[]=[];
  members$:Observable<Member[]>|undefined
 pagination:Pagination ;
 userParams:userParams;
//  user:User ;
 genderList=[{value:'male',display:'Males'},{value:'female',display:'Females'}]
 
  constructor(private membersService:MembersService)
  {
    this.userParams=this.membersService.getUserParams();
    // this.accountService.currentUser$.pipe(take(1)).subscribe({
    //   next:user=>{
    //     if(user)
    //     {
    //       this.userParams=new userParams(user);
    //       this.user=user;
    //     }
    //   }
    // })

    //above all code added in member services constructor


  }
  ngOnInit(): void {
    //  this.members$=this.membersService.getMembers();
    this.loadmembers();
  }
  // applyFilter()
  // {
  //  if(this.userParams)
  //  {
  //       if(this.pagination)
  //       {
  //         this.userParams.pageNumber=1;

  //       }
  //       this.userParams.pageNumber=1;
  //       this.membersService.setUserParams(this.userParams);
  //       this.loadmembers();
  //  }
  // }
  loadmembers()
  {

   
    if(this.userParams)
    {
      this.membersService.setUserParams(this.userParams);
      this.membersService.getMembers(this.userParams).subscribe({
        next:response=>{
          if(response.result && response.pagination)
          {
            this.members=response.result;
            this.pagination=response.pagination;
          }
        }
      })
    }
  }
  resetFilters()
  {
    this.userParams=this.membersService.resetUserParams();
    this.loadmembers();
 
  }
  // apply()
  // {
  //   // if(this.userParams)
  //   // {
  //   //   if(this.pagination)
  //   //   {
  //   //     this.pagination.currentPage=1;

  //   //   }
  //     this.userParams.pageNumber=1;
  //     this.membersService.setUserParams(this.userParams);
  //     this.loadmembers();
  //   // }
  // }

  pageChanged(event:any)
  {
    if(this.userParams &&  this.userParams?.pageNumber!==event.page)
    {

      this.userParams.pageNumber=event.page;
      this.membersService.setUserParams(this.userParams);
      this.loadmembers();
    }

  }

  // loadMembers()
  // {
  //   this.membersService.getMembers().subscribe({
  //     next:members=>this.members=members
  //   })
  // }
}
