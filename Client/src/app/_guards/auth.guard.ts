import { CanActivateFn } from '@angular/router';
import { AccoutnService } from '../_services/accoutn.service';
import { inject } from '@angular/core';
import { ToastrService } from 'ngx-toastr';
import { map } from 'rxjs';

export const authGuard: CanActivateFn = (route, state) => {
const accountservice =inject (AccoutnService);
const toastr=inject (ToastrService);

  return accountservice.currentUser$.pipe(
    map(user=>{
      if(user) return true;
      else
      {
        toastr.error("you shall not pass");
        return false;
      }
    })
  )
};