import { Component, OnInit } from '@angular/core';
import { environment } from 'src/environments/environment';
import { AuthService } from './services/authSerivce';
import { TenantService } from './services/tenant.service';

@Component({
  selector: 'app-root',
  templateUrl: './app.component.html',
  styleUrls: ['./app.component.scss']
})
export class AppComponent implements OnInit {
  title = 'visitors';
  tenant: string = "";
  isTenantValid = true;
  isLoading = true;
  isMultiTenant = environment.isMultiTenant;
  constructor(private _tenant: TenantService, private _auth: AuthService) {
    this.tenant = this._tenant.getTenant();
  }
  // ngOnInit(): void {
  //   if (!this.isMultiTenant) {
  //     this.isTenantValid = true;
  //     this.isLoading = false;
  //     return;
  //   }

  //   setTimeout(() => {
  //     this.validateTenant();
  //   }, 2000);
  // }

  ngOnInit(): void {

  // FORCE BYPASS FOR DOCKER TESTING
  this.isMultiTenant = false;
  this.isTenantValid = true;
  this.isLoading = false;

  return;
}

  validateTenant() {
    this._auth.validatedTenant(this.tenant).subscribe(res => {
      this.isTenantValid = true;
      this.isLoading = false;
    }, error => {
      this.isTenantValid = false;
      this.isLoading = false;
    })
  }
}
