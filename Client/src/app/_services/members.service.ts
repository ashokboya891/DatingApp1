import { HttpClient, HttpHeaders, HttpParams } from '@angular/common/http';
import { Injectable } from '@angular/core';
import { environment } from 'src/environments/environment';
import { Member } from '../_models/member';
import { ReturnStatement } from '@angular/compiler';
import { elementAt, map, of, take } from 'rxjs';
import { PaginatedResult } from '../_models/pagination';
import { userParams } from '../_models/userParams';
import { AccoutnService } from './accoutn.service';
import { User } from '../_models/User';

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
  memberCache=new Map();
  user:User |undefined;
  userParams:userParams|undefined;
  
  
  // paginatedResult:PaginatedResult<Member[]>=new PaginatedResult<Member[]>

    constructor(private http:HttpClient,private accountService:AccoutnService)
    { 
      this.accountService.currentUser$.pipe(take(1)).subscribe({
        next:user=>{
          if(user)
          {
            this.userParams=new userParams(user);
            this.user=user;
          }
        }
      })
    }
    getUserParams()
    {
      return this.userParams;
    }
    setUserParams(params:userParams)
    {
      this.userParams=params;
    }
    getMembers(userParams:userParams)
    {
      // console.log(Object.values(userParams).join('-'));

      const response=this.memberCache.get(Object.values(userParams).join('-'));
      if(response)return of(response);
      let params = this.getPaginationHeader(userParams.pageNumber,userParams.pageSize);

      params=params.append('minAge',userParams.minAge.toString());
      params=params.append('maxAge',userParams.maxAge.toString());
      params=params.append('gender',userParams.gender);
      params=params.append('orderBy',userParams.orderBy);

      return this.getPaginatedResult<Member[]>(this.baseUrl+'users',params).pipe(
        map(response=>{
          this.memberCache.set(Object.values(userParams).join('-'),response);
          return response;
        })
      )

    }
    resetUserParams()
    {
      
      if(this.user)
      {
        this.userParams=new userParams(this.user);
        return this.userParams;
        console.log('in reset');

      }
      return null;
    }
  
    getMember(username:string)
    {
    //  console.log(this.memberCache);
     
      // const members=this.members.find(x=>x.userName===username);
      // if(members)return of(members);

      const member=[...this.memberCache.values()].reduce((arr,elem)=>arr.concat(elem.result),[])
      .find((member:Member)=>member.userName===username);
      if(member)return of(member);
       
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

    setMainPhoto(photoId:number)
    {
      return this.http.put(this.baseUrl+'users/set-main-photo/'+photoId,{});
    }
    DeletePhoto(photoId:number)
    {
      return this.http.delete(this.baseUrl+'users/delete-photo/'+photoId);

    }
    private getPaginatedResult<T>(url:string,params:HttpParams) {

      const paginatedResult:PaginatedResult<T>=new PaginatedResult<T>;
      return this.http.get<T>(url , { observe: 'response', params }).pipe(
        map(response => {
         
  
        if(response.body)
        {
          paginatedResult.result = response.body;
        }
        const pagination=response.headers.get('pagination');
          if(pagination)
          {
  
            paginatedResult.pagination = JSON.parse(pagination);
          }
         return paginatedResult;
        })
      );
    }
  
    private getPaginationHeader(pageNumber:number,pageSize:number) {
      let params = new HttpParams();
     
        params = params.append('pageNumber', pageNumber.toString());
        params = params.append('pageSize', pageSize.toString());
      
      return params;
    }
  
  }


  // gmembers{
      // if(this.members.length>0)return of(this.members);
      // return this.http.get<Member[]>(this.baseUrl+ 'users').pipe(
          // map(members=>{
          //   this.members=members;
          //   return members;
          // })
      // )


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
    // }

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

