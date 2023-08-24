import { User } from "./User";

export class userParams{
    gender:string;
    minAge=18;
    maxAge=99;
     pageNumber=1;
    pageSize=12
    ;
    orderBy='lastActive';
    constructor(user:User)
    {
        this.gender=user.gender==='female'?'male':'female';
    }

}