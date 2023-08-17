import { HttpClient, HttpHeaders } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';
import { ReturnStatement } from '@angular/compiler';
import { map, of } from 'rxjs';

// const HttpOptions={
//   headers:new HttpHeaders({
//     Authorization:'Bearer '+JSON.parse(localStorage.getItem('user')).token
//   })

// }
@Injectable({
  providedIn: 'root'
})

export class MembersService {
  baseUrl=environment.apiUrl;
  members:Member[]=[];
  constructor(private http:HttpClient)
   {

    }

    getMembers()
    {
      if(this.members.length>0)return of(this.members);
      return this.http.get<Member[]>(this.baseUrl+ 'users').pipe(
          map(members=>{
            this.members=members;
            return members;
          })
      )
    }
    getMember(username:string)
    {
      const members=this.members.find(x=>x.userName===username);
      if(members)return of(members);
      return this.http.get<Member>(this.baseUrl+ 'users/'+username);
    }
    updateMember(member:Member)
    {
        return this.http.put(this.baseUrl+'users',member).pipe(
          map(()=>{
            const index=this.members.indexOf(member);
            this.members[index]={...this.members[index],...member}
          })
        )
    }

    // getHttpOptions()
    // {
    //   const userString=localStorage.getItem('user');
    //   if(!userString) return {};
    //   const user=JSON.parse(userString);
    //   return{
    //     headers:new HttpHeaders({
    //       Authorization:'Bearer '+user.token
    //     })
    //     }
    //   }
    }

    // getMember(username:string)
    // {
    //   return this.http.get<Member[]>(this.baseUrl+ 'users/'+username, HttpOptions);

    // }
    // getHttpOptions()
    // {
    //   const userString = localStorage.getItem('user');
    //   if(!userString) return null;
       

    //     const user=JSON.parse(userString);
    //     return {
        
    //     }
      
     
      


    // }

