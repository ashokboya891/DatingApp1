import { Injectable } from '@angular/core';
import { HubConnection, HubConnectionBuilder } from '@microsoft/signalr';
import { ToastrService } from 'ngx-toastr';
import { environment } from "src/environments/environment";
import { User } from '../_models/User';
import { BehaviorSubject, take } from 'rxjs';
import { Router } from '@angular/router';
@Injectable({
  providedIn: 'root'
})
export class PresenceService {
  hubUrl=environment.hubUrl
  private hubConnection?:HubConnection;
  private onlineUserSource=new BehaviorSubject<string[]>([]);
  onlineUsers$=this.onlineUserSource.asObservable();


  constructor(private toastr:ToastrService,private router:Router) { }

  createHubConnection(user:User)
  {
        this.hubConnection=new HubConnectionBuilder()
        .withUrl(this.hubUrl+'presence',{
          accessTokenFactory:()=>user.token
        })
        .withAutomaticReconnect()
        .build();
        this.hubConnection.start().catch(error=>console.log(error));

        this.hubConnection.on('UserIsOnline',username=>{
          this.onlineUsers$.pipe(take(1)).subscribe({
            next:usernames=>this.onlineUserSource.next([...usernames,username])
             // this.toastr.info(username+'has connected');
          })
        })

        this.hubConnection.on("UserIsOffline",username=>{
          this.onlineUsers$.pipe(take(1)).subscribe({
            next:usernames=>this.onlineUserSource.next(usernames.filter(x=>x!==username))
            // this.toastr.warning(username+'has disconnected')
          })
        })
        this.hubConnection.on('GetOnlineUsers',usernames=>{
          this.onlineUserSource.next(usernames);
        })
        this.hubConnection.on('NewMessageReceived',({username,knownAs})=>{
          
          this.toastr.info(knownAs+'has sent you a new Message !Click  me to see it ')
          .onTap
          .pipe(take(1))
          .subscribe({
            next:()=>this.router.navigateByUrl('/members/'+username+'?tab=Messages')
          })

        })
  }
  stopHubConnection()
  {
    this.hubConnection?.stop().catch(error=>console.log(error));
    
  }
}
