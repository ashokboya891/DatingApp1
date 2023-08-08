import { HttpClient } from '@angular/common/http';
import { Component,OnInit } from '@angular/core';

@Component({
  selector: 'app-home',
  templateUrl: './home.component.html',
  styleUrls: ['./home.component.css']
})
export class HomeComponent  implements OnInit{
  users:any;
  registerMode=false;
  constructor(private http:HttpClient){

  }
  ngOnInit(): void {
      this.getUser();
  }
  registerToggle()
  {
    this.registerMode=!this.registerMode;
  }
  
  getUser()
  {
    this.http.get('https://localhost:5001/api/users').subscribe({
      next:Response=>this.users=Response,
      error:error=>console.log(error),
      complete:()=>console.log("request completed....")
    })
  }
  cancelRegisterMode(Event:boolean)
  {
    this.registerMode=Event;
  }
}
