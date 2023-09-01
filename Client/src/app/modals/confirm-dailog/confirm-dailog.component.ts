import { Component, OnInit } from '@angular/core';
import { BsModalRef } from 'ngx-bootstrap/modal';

@Component({
  selector: 'app-confirm-dailog',
  templateUrl: './confirm-dailog.component.html',
  styleUrls: ['./confirm-dailog.component.css']
})
export class ConfirmDailogComponent implements OnInit {
  title='';
  message='';
  btnOktext='';
  btnCancelText='';
  result=false;

  constructor(public bsModalRef:BsModalRef)
  {

  }

  ngOnInit(): void {
    throw new Error('Method not implemented.');
  }
  confirm()
  {
    this.result=true;
    this.bsModalRef.hide();
  }
  decline()
  {
    this.bsModalRef.hide();
  }

}
