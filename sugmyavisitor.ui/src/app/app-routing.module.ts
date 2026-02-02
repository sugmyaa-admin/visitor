import { NgModule } from '@angular/core';
import { RouterModule, Routes } from '@angular/router';
import { LoginComponent } from './component/login/login.component';
import { ResetPasswordComponent } from './component/reset-password/reset-password.component';
import { ContainerComponent } from './component/container/container.component';
// import { DashboardComponent } from './component/dashboard/dashboard.component';
import { AuthGuard } from './auth.guard';
import { ResetPasswordAccessGuard } from './component/reset-password/reset-password-access.guard';
import { ResponseComponent } from './component/response/response.component';


const routes: Routes = [
  { path: '', redirectTo: 'login', pathMatch: 'full' },
  // { path: '', component: LoginComponent },
  { path: 'login', component: LoginComponent },
  { path: 'response', component: ResponseComponent},
  { path: 'reset-password', component: ResetPasswordComponent,canActivate: [ResetPasswordAccessGuard]},
  {
    path: '', component: ContainerComponent, // Use container here for the main layout
    children: [
     
      { path: 'dashboard',
        loadChildren: () => import('./component/dashboard/dashboard.module').then(m => m.DashboardModule), // Lazy load DashboardModule
          canActivate: [AuthGuard]
      }
     
    ]
  }

];


@NgModule({
  imports: [RouterModule.forRoot(routes)],
  exports: [RouterModule]
})
export class AppRoutingModule { }
