

import { Component, OnInit } from '@angular/core';
import { FormBuilder, FormGroup, Validators } from '@angular/forms';
import { Router } from '@angular/router';
import { VisitorService } from '../../services/visitor.service'; // Import VisitorService
import { ToastrService } from 'ngx-toastr'; // Import ToastrService

@Component({
  selector: 'app-reset-password',
  templateUrl: './reset-password.component.html',
  styleUrls: ['./reset-password.component.scss']
})
export class ResetPasswordComponent implements OnInit {

  loginForm: FormGroup;
  showNewInput = false;
  otpConfirmed = false;
  buttonLabel = 'Reset';
  showCard = false;
  otpSent = false;

  constructor(
    private fb: FormBuilder,
    private router: Router,
    private visitorService: VisitorService, // Inject VisitorService
    private toastr: ToastrService // Inject ToastrService
  ) {
    this.loginForm = this.fb.group({
      mobileNo: ['', [Validators.required,Validators.minLength(10)]],
      newInput: [''],
      newPassword: [''],
      confirmPassword: ['']
    });
  }

  ngOnInit(): void { }

  toggleReset(event: Event) {
    event.preventDefault();
    if (!this.showNewInput) {
      this.showNewInput = true;
      this.buttonLabel = 'Confirm';
      this.loginForm.get('newInput')?.setValidators([Validators.required]);
      this.loginForm.get('newInput')?.updateValueAndValidity();
      this.showCard = true;

      this.sendOtpNumber(); // Call the API to send OTP
    } else if (!this.otpConfirmed && this.loginForm.get('newInput')?.valid) {
      this.confirmOtp(); // Call the API to confirm OTP
    } else if (this.otpConfirmed) {
      if (this.loginForm.valid) {
        const newPassword = this.loginForm.get('newPassword')?.value;
        const confirmPassword = this.loginForm.get('confirmPassword')?.value;
        if (newPassword === confirmPassword) {
          this.resetPassword(); // Call the API to reset the password
        } else {
          this.toastr.error('Passwords do not match', 'Error'); // Show error if passwords do not match
        }
      }
    }
  }
 
  sendOtpNumber() {
    const mobileNo = this.loginForm.get('mobileNo')?.value?.toString();
    console.log("User", mobileNo);

    this.visitorService.sendOtp(mobileNo).subscribe({
      next: (response: any) => {
        this.toastr.success('OTP sent successfully!', 'Success'); // Show success notification
        this.otpSent = true;
      },
      error: (error: any) => {
        this.toastr.error('Error sending OTP', 'Error');
        // Show error notification
      }
    });
  }

  confirmOtp() {
    const otp = this.loginForm.get('newInput')?.value.toString();
    const mobileNo = this.loginForm.get('mobileNo')?.value.toString();
    console.log("Confirm",otp,mobileNo);
    this.visitorService.confirmOtp(mobileNo, otp).subscribe({
      next: (response: any) => {
        this.toastr.success('OTP confirmed successfully!', 'Success'); // Show success notification
        this.otpConfirmed = true;
        this.buttonLabel = 'Submit';
        this.loginForm.get('newPassword')?.setValidators([Validators.required, Validators.minLength(6)]);
        this.loginForm.get('confirmPassword')?.setValidators([Validators.required, Validators.minLength(6)]);
        this.loginForm.get('newPassword')?.updateValueAndValidity();
        this.loginForm.get('confirmPassword')?.updateValueAndValidity();
      },
      error: (error: any) => {
        this.toastr.error('Invalid OTP', 'Error'); // Show error notification if OTP is wrong
      }
    });
  }

  resetPassword() {
    const newPassword = this.loginForm.get('newPassword')?.value;
    const mobileNo = this.loginForm.get('mobileNo')?.value;
    this.visitorService.resetPassword(mobileNo, newPassword).subscribe({
      next: (response: any) => {
        this.toastr.success('Password reset successfully!', 'Success'); // Show success notification
        this.router.navigate(['']);
      },
      error: (error: any) => {
        this.toastr.error('Error resetting password', 'Error'); // Show error notification
      }
    });
  }
  hideCard() {
    this.showCard = false;  // Hide the card when the button is clicked
  }
  backToLogin() {
    this.router.navigate(['']);
  }

  onSubmit() {
    // Handle form submission logic here
    if (this.loginForm.valid) {
      console.log(this.loginForm.value);
    }
  }
}
