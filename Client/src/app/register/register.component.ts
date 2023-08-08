import { Component, OnInit,Input, EventEmitter,Output } from '@angular/core';
import { AccoutnService } from '../_services/accoutn.service';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  // @Input() usersFromHomeComponent:any;
  @Output() cancelRegister=new EventEmitter();
  model:any={}
  constructor(private accountService:AccoutnService) {
    
  }
  ngOnInit(): void {
      
  }

  register()
  {
    this.accountService.register(this.model).subscribe({
      next:response=>{
        // console.log(response);
        this.cancel();
        
      },
      error:error=>console.log(error)
      
    });
    
  }
  cancel()
  {
    this.cancelRegister.emit(false);
  }

}
