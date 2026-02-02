import { NgModule } from '@angular/core';
import { CommonModule } from '@angular/common';
import { FormsModule } from '@angular/forms';  // Import FormsModule for ngModel
import { RouterModule, Routes } from '@angular/router';
import { NgbDatepickerModule, NgbTooltipModule, NgbPopoverModule, NgbTypeaheadModule, NgbDateParserFormatter } from '@ng-bootstrap/ng-bootstrap';
import { DashboardComponent } from './dashboard.component';
import { NgbDateCustomParserFormatter } from '../../date-format';  // Import custom date formatter
 import { SpinDirective } from 'src/app/common/appDirectives/spin.directive';
import { ImageDialogComponent } from 'src/app/common/image-dialog/image-dialog.component';
import { MatTableModule } from '@angular/material/table';
import { MatDialogModule } from '@angular/material/dialog';


const routes: Routes = [
  { path: '', component: DashboardComponent }
];

@NgModule({
  declarations: [
    DashboardComponent,
     SpinDirective,
    ImageDialogComponent,
  ],
  imports: [
    CommonModule,
    FormsModule,  // Add FormsModule here
    NgbDatepickerModule,
    NgbTooltipModule,
    NgbPopoverModule,
    NgbTypeaheadModule,
    MatTableModule,
    MatDialogModule,
    
    RouterModule.forChild(routes)
  ],
  providers: [
    {
      provide: NgbDateParserFormatter,
      useClass: NgbDateCustomParserFormatter
    }
  ],
  entryComponents: [ImageDialogComponent]
})
export class DashboardModule { }
