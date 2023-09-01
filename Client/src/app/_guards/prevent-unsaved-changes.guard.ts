import { CanDeactivateFn } from '@angular/router';
import { MemberEditComponent } from '../members/member-edit/member-edit.component';
import { ConfirmService } from '../_services/confirm.service';
import { inject } from '@angular/core';

export const preventUnsavedChangesGuard: CanDeactivateFn<MemberEditComponent> = (component) => {
  const confirmService=inject(ConfirmService);
  if(component.editForm.dirty)
  {
    return confirmService.confirm();
    
    // return confirm('are you sure you want to continue ? any unsaved changes will be lost')  this line commented afer adding confirmservice and confirm component dailog
  }
  return true;
};
