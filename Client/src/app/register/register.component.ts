import { Component, OnInit,Input, EventEmitter,Output } from '@angular/core';
import { AccoutnService } from '../_services/accoutn.service';
import { ToastrService } from 'ngx-toastr';
import { AbstractControl, FormBuilder, FormControl, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';

@Component({
  selector: 'app-register',
  templateUrl: './register.component.html',
  styleUrls: ['./register.component.css']
})
export class RegisterComponent implements OnInit {
  [x: string]: any;
  // @Input() usersFromHomeComponent:any;
  @Output() cancelRegister=new EventEmitter();
  registerForm:FormGroup=new FormGroup({});
  maxDate:Date=new Date();
  validationErrors:string [] |undefined;



  // model:any={}
  constructor(private accountService:AccoutnService,private toastr:ToastrService,private fb:FormBuilder,
    private router:Router) {
    
  }
  ngOnInit(): void {

    this.initializeForm();
      this.maxDate.setFullYear(this.maxDate.getFullYear()-18);
  }
    initializeForm()
    {
      this.registerForm=this.fb.group({
        gender:['male'],
        username:['',Validators.required],
        KnownAs:['',Validators.required],
        dateOfBirth:['',Validators.required],
        city:['',Validators.required],
        country:['',Validators.required],
        password:['',[Validators.required,Validators.minLength(4),Validators.maxLength(8)]],
        confirmPassword:['',[Validators.required,this.matchvalues('password')]]

      });
      this.registerForm.controls['password'].valueChanges.subscribe({
        next:()=>this.registerForm.controls['confirmPassword'].updateValueAndValidity()
      })
    }
    matchvalues(matchTo:string)
    {
      return (control:AbstractControl)=>{
        return control.value===control.parent?.get(matchTo)?.value?null:{notmaching:true}
      }
    }

  register()
  {
    console.log(this.registerForm.value);
    
   const dob=this.getDateonly(this.registerForm.controls['dateOfBirth'].value);
   const values={...this.registerForm.value,dateOfBirth:dob};
   
    // console.log(values);
    
    this.accountService.register(values).subscribe({
      next:response=>{
        this.router.navigateByUrl('/members')
        
      },
      error:error=>{
        // this.toastr.error(error.error);
        // console.log(error);
        this.validationErrors=error
        
      }
      
    });
    
  }
  cancel()
  {
    this.cancelRegister.emit(false);
  }
  private getDateonly(dob:string|undefined)
  {
    if(!dob)return {};
    let thedob=new Date(dob);
    return new Date(thedob.setMinutes(thedob.getMinutes()-thedob.getTimezoneOffset())).toISOString().slice(0,10);
  }

}
