import { Directive, Input, OnInit, TemplateRef, ViewContainerRef } from '@angular/core';
import { User } from '../_models/User';
import { AccoutnService } from '../_services/accoutn.service';
import { take } from 'rxjs';

@Directive({
  selector: '[appHasRole]' //*apphasRole='["Admin","thing"]
})
export class HasRoleDirective  implements OnInit{
  @Input() appHasRole:string[]=[];
  user:User={} as User;

  constructor(private viewContainerRef:ViewContainerRef,
    private templateRef:TemplateRef<any>,private accountService:AccoutnService) 
    {
        this.accountService.currentUser$.pipe(take(1)).subscribe({
          next:user=>{
            if(user)this.user=user;
          }
        })

    }
    ngOnInit(): void {
      // debugger;

        if(this.user.roles.some(r=>this.appHasRole.includes(r))){
          this.viewContainerRef.createEmbeddedView(this.templateRef);

        }else{
          this.viewContainerRef.clear();
        }
    }

}
