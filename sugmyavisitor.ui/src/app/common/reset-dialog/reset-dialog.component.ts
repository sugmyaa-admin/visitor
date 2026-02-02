import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { MatDialogRef } from '@angular/material/dialog';
import { VisitorService } from '../../services/visitor.service';
import { ToastrService } from 'ngx-toastr'; 
import { Router } from '@angular/router';

@Component({
  selector: 'app-reset-dialog',
  templateUrl: './reset-dialog.component.html',
  styleUrls: ['./reset-dialog.component.scss']
})
export class ResetDialogComponent implements OnInit {
  loginForm: FormGroup;
  constructor(private fb: FormBuilder,
    public dialogRef: MatDialogRef<ResetDialogComponent>,private visitorService: VisitorService,
    private toastr: ToastrService,private router: Router) { 
    this.loginForm = this.fb.group({
      oldPassword: ['',[Validators.required, Validators.minLength(6)]],
      password: ['', [Validators.required, Validators.minLength(6)]],
      
    });
  }

  ngOnInit(): void {
    // this.dialogRef.close(); 
  }
  onClose(): void {
    this.dialogRef.close(); 
  }
  onSubmit(): void {
    if (this.loginForm.valid) {
      let { oldPassword, password } = this.loginForm.value;
     
      const token=sessionStorage.getItem('authToken');
       if(token){
      this.visitorService.newPassword(oldPassword,password).subscribe({
        next: (response: any) => {
        
         if(response.status.success==true){
          this.toastr.success('password Change successfully!', 'Success');
          this.router.navigate([""]);
          this.onClose();
          sessionStorage.clear();
         }
         else{
          this.toastr.error(response.status.message, 'Error'); 
         }
          
          
          
        },
        error: (error: any) => {
          this.toastr.error('Error setting password', 'Error');
          // Show error notification
        }
      });
    } else {
      // Handle form invalid case, e.g., show validation messages
      console.error('Form is invalid');
    }
  }
}
}
