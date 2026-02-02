
import { NgModule } from '@angular/core';
import { BrowserModule } from '@angular/platform-browser';
import { BrowserAnimationsModule } from '@angular/platform-browser/animations';
import { AppRoutingModule } from './app-routing.module';
import { AppComponent } from './app.component';
import { LoginComponent } from './component/login/login.component';
import { ReactiveFormsModule } from '@angular/forms';
import { HttpClientModule, HTTP_INTERCEPTORS } from '@angular/common/http';
import { ResetPasswordComponent } from './component/reset-password/reset-password.component';
import { FormsModule } from '@angular/forms';
import { ContainerComponent } from './component/container/container.component';
import { NgbDatepickerModule, NgbTooltipModule, NgbPopoverModule, NgbTypeaheadModule, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
import { NgbDateCustomParserFormatter } from './date-format';
import { ToastrModule } from 'ngx-toastr';
import { VisitorService } from './services/visitor.service';
import { AuthInterceptor } from './common/loading/loading.interceptor';
import { LoadingComponent } from './common/loading/loading.component';
import { MatToolbarModule } from '@angular/material/toolbar';
import{MatDialogModule} from '@angular/material/dialog'
import { ResetDialogComponent } from './common/reset-dialog/reset-dialog.component';
import { ResponseComponent } from './component/response/response.component';
import { LoadingSpinnerComponent } from './common/loading-spinner/loading-spinner.component';
@NgModule({
  declarations: [
    AppComponent,
    LoginComponent,
    ResetPasswordComponent,
    ContainerComponent,
    LoadingComponent,
    ResetDialogComponent,
    ResponseComponent,
    LoadingSpinnerComponent
  ],
  imports: [
    BrowserModule,
    AppRoutingModule,
    ReactiveFormsModule,
    BrowserAnimationsModule,
    HttpClientModule,
    FormsModule,
    NgbDatepickerModule,
    NgbTooltipModule,
    NgbPopoverModule,
    NgbTypeaheadModule,
    // DashboardModule,
    MatToolbarModule,
    MatDialogModule,
    ToastrModule.forRoot({
      timeOut: 3000, // Toast will disappear after 3 seconds
      positionClass: 'toast-top-right', // Position of the toast notification
      preventDuplicates: true, // Avoid duplicate toasts
    }),
  ],
  providers: [
    VisitorService,
    {
      provide: HTTP_INTERCEPTORS,
      useClass: AuthInterceptor,
      multi: true
    },
    {
      provide: NgbDateParserFormatter,
      useClass: NgbDateCustomParserFormatter
    },
  ],
  bootstrap: [AppComponent],
})
export class AppModule { }
