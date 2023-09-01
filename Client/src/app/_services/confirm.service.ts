import { Injectable } from '@angular/core';
import { BsModalRef, BsModalService } from 'ngx-bootstrap/modal';
import { ConfirmDailogComponent } from '../modals/confirm-dailog/confirm-dailog.component';
import { Observable, map } from 'rxjs';

@Injectable({
  providedIn: 'root'
})
export class ConfirmService {
  bsModalRef?:BsModalRef<ConfirmDailogComponent>;

  constructor(private modalService:BsModalService) 
  {


  }
  confirm(
    title='Confirmation',
    message='Are you sure you want to do this ?',
    btnOktext='Ok',
    btnCancelText='Cancel',
    ):Observable<boolean>{
      const config={
        initialState:{
          title,
          message,
          btnOktext,
          btnCancelText
        }
      }
      this.bsModalRef=this.modalService.show(ConfirmDailogComponent,config);
      return  this.bsModalRef.onHidden!.pipe(
        map(()=>{
           return this.bsModalRef!.content!.result
        })
      )
  }
}
