import { Component, OnInit, AfterViewInit } from '@angular/core';
import { Router } from '@angular/router';
import { MatDialog } from '@angular/material/dialog';
import { ResetDialogComponent } from 'src/app/common/reset-dialog/reset-dialog.component';
import { ToastrService } from 'ngx-toastr';
import { environment } from 'src/environments/environment';
import { VisitorService } from 'src/app/services/visitor.service';
declare var $: any;
@Component({
  selector: 'app-container',
  templateUrl: './container.component.html',
  styleUrls: ['./container.component.scss']
})
export class ContainerComponent implements OnInit {
  personName: string = '';
  personEmail: string = '';
  initials: string = '';
  appVersion = environment.appVersion
  constructor(private router: Router, public dialog: MatDialog,
    private totstr: ToastrService, private _visitor: VisitorService, private toastr: ToastrService,) { }

  ngOnInit(): void {
    this.fetchuser();

  }


  ngAfterViewInit(): void {
    setTimeout(() => {
      $("#userName").click(function (evt: any) {
        $("#userName").toggleClass("show")
        evt.stopPropagation();
      })
      $(document).click(function (e: any) {
        $("#userName").removeClass("show")
      })
    }, 1000);
  }

  getInitials(name: string): string {
    const nameParts = name.split(' ');
    const firstInitial = nameParts[0]?.charAt(0).toUpperCase() || '';

    const secondInitial = nameParts[1]?.charAt(0).toUpperCase() || '';
    return `${firstInitial}${secondInitial}`;
  }
  fetchuser() {
    this._visitor.getUser().subscribe({
      next: (response: any) => {

        if (response) {


          this.personName = response.data[0].name;
          this.personEmail = response.data[0].email_id;
          this.initials = this.getInitials(this.personName);
        }
      },
      error: (err: any) => {

        this.toastr.error('Failed to fetch user data.', err);
      }
    });
  }



  navigateToResetPassword() {
    sessionStorage.removeItem('authToken');
    this.totstr.success("Logout Successfully")
    this.router.navigate(['']);
  }
  openResetDialog(): void {
    this.dialog.open(ResetDialogComponent, {
      width: '300px',
      // data:null,
      panelClass: 'custom-dialog-container'
    });
  }
}
