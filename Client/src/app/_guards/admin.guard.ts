import { CanActivateFn } from '@angular/router';
import { AccoutnService } from '../_services/accoutn.service';
import { inject } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { map } from 'rxjs';

export const adminGuard: CanActivateFn = (route, state) => {
  const accountService=inject(AccoutnService);
  const toastr=inject(ToastrService);


  return accountService.currentUser$.pipe(
    map(user=>{
      // debugger
      if(!user)return false;
      if(user.roles.includes('Admin')|| user.roles.includes('Moderator'))
      {
        return true;
      }
      else{
        toastr.error('you cannot enter this area');
        return false;
      }
    })
  );
};
