import { Component,OnInit } from '@angular/core';
import { AccoutnService } from '../_services/accoutn.service';
import { Observable, of } from 'rxjs';
import { User } from "../_models/User";
@Component({
  selector: 'app-nav',
  templateUrl: './nav.component.html',
  styleUrls: ['./nav.component.css']
})
export class NavComponent implements OnInit {

  model:any={}
  // loggedIn=false;
  // currentUser$:Observable<User|null>=of(null)

  constructor(public accountService:AccoutnService)
  {
    
  }

  ngOnInit(): void {
  //  this.currentUser$=this.accountService.currentUser$;   
  }
  login()
  {
  this.accountService.login(this.model).subscribe({
    next:response=>{
      console.log(response);
      
      },
    error:error=>console.log(error)  
  })
 }



 logout()
 {
   this.accountService.logout();
 
 }
 }

