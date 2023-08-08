
import { Component ,OnInit} from '@angular/core';
import { FormBuilder } from '@angular/forms';
import { AccoutnService } from './_services/accoutn.service';
import { User } from "./_models/User";
@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.css']
})
export class AppComponent implements OnInit{
  title = 'App';


  constructor(private accountService:AccoutnService)
  {
     
  }
  ngOnInit(): void {

  this.setCurrentUser();
  }



  setCurrentUser()
  {
    // const user:User=JSON.parse(localStorage.getItem('user'));

    const userString=localStorage.getItem('user');

    if(!userString) return ;
    const user:User=JSON.parse(userString);
    this.accountService.setCurrentUser(user);
   
  }
}
