import { Component, Input, OnInit } from '@angular/core';
import { MatDialog } from '@angular/material/dialog';
import { ToastrService } from 'ngx-toastr';
import { ImageDialogComponent } from 'src/app/common/image-dialog/image-dialog.component';
import { VisitorService } from 'src/app/services/visitor.service';
import * as ExcelJS from 'exceljs';
import * as FileSaver from 'file-saver';
declare var $: any;
@Component({
  selector: 'app-dashboard',
  templateUrl: './dashboard.component.html',
  styleUrls: ['./dashboard.component.scss']
})
export class DashboardComponent implements OnInit {
  isLoading = false;
  startDate: any;
  isDisabled: boolean = true;
  endDate: any;
  now = new Date();
  isAllSelected = false;
  isInde = false;
  selectedRows: any = [];
  table: any;
  data: any = [];
  filteredData: any = [];

  constructor(private _visitor: VisitorService, public dialog: MatDialog, private toastr: ToastrService) {
    const startOfMonth = new Date(this.now.getFullYear(), this.now.getMonth(), 1);
    this.startDate = { year: this.now.getFullYear(), month: this.now.getMonth() + 1, day: this.now.getDate() };
    this.endDate = { year: this.now.getFullYear(), month: this.now.getMonth() + 1, day: this.now.getDate() };
  }

  ngOnInit(): void {
    this.getVisitors();

  }

  getVisitors() {
    this.isLoading = true;
    this._visitor.getVisitors().subscribe(res => {
      if (res.status.success) {
        this.data = res.data;
      } else {
        this.data = [];
      }
      this.onSearch();
    }, error => {
      this.isLoading = false;
    });
  }



  onSelectAll() {
    this.data.forEach((item: any) => item.checked = this.isAllSelected)
    this.getSelectedRows()
  }
  change(event: any) {
    var check = this.data.filter((x: any) => x.checked).length
    if (event.target.checked) {
      if (this.data.length == check) {
        this.isAllSelected = true;
        this.isInde = false;
      }
      else {
        this.isAllSelected = false;
        this.isInde = true;

      }
    }
    else {
      this.isAllSelected
      this.isInde = true;
      if (check == 0) {
        this.isAllSelected = false;
        this.isInde = false;
      }
    }
    this.getSelectedRows()
  }
  getSelectedRows() {
    var dt = this.data.filter((x: any) => x.checked);
    this.selectedRows = dt;
  }
  format(date: any) {
    var months: any = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"];
    return String(date.month).padStart(2, '0') + "/" + String(date.day).padStart(2, '0') + "/" + date.year
  }
  downloadCSV() {
    // Create a new workbook and add a worksheet
    const workbook = new ExcelJS.Workbook();
    const worksheet = workbook.addWorksheet('Visitors Data');

    // Add headers to the worksheet
    worksheet.columns = [
      { header: 'SNo', key: 'sno', width: 5 },
      { header: 'Visitor_Picture', key: 'picture', width: 30 },
      { header: 'Visitor_Name', key: 'name', width: 20 },
      { header: 'Check_In_Time', key: 'checkIn', width: 20 },
      { header: 'Check_Out_Time', key: 'checkOut', width: 20 },
      { header: 'Visitor_Email', key: 'email', width: 30 },
      { header: 'Visitor_Number', key: 'number', width: 15 },
      { header: 'Purpose_Of_Visit', key: 'purpose', width: 20 },
      { header: 'No_Of_Person', key: 'numberOfPerson', width: 15 },
      { header: 'Whom_To_Meet', key: 'whomToMeet', width: 20 }
    ];

    // Add visitor data and hyperlink to their picture
    this.filteredData.forEach((visitor: any, index: number) => {
      const row = worksheet.addRow({
        sno: index + 1,
        name: visitor.name,
        checkIn: visitor.checkIn,
        checkOut: visitor.checkOut,
        email: visitor.email,
        number: visitor.number,
        purpose: visitor.purpose,
        numberOfPerson: visitor.numberOfPerson,
        whomToMeet: visitor.whomToMeet,
      });

      // Add the visitor's name as a hyperlink to their picture
      if (visitor.picture) {
        worksheet.getCell(`B${row.number}`).value = {
          text: `${visitor.name} - Image`, // Concatenate visitor's name with "Image"
          hyperlink: visitor.picture // Hyperlink to the visitor's picture
        };
        worksheet.getCell(`B${row.number}`).font = {
          color: { argb: 'FF0000FF' }, // Blue color
          underline: true // Underline to indicate it's a hyperlink
        };
      }

    });
    const getMonthShortName = (monthIndex: number) => {
      const months = ['Jan', 'Feb', 'Mar', 'Apr', 'May', 'Jun', 'Jul', 'Aug', 'Sep', 'Oct', 'Nov', 'Dec'];
      return months[monthIndex];
    };


    const now = new Date();
    const dayStr = now.getDate().toString().padStart(2, '0');
    const monthStr = getMonthShortName(now.getMonth());
    const yearStr = now.getFullYear().toString();
    const randomNum = Math.floor(1000 + Math.random() * 9000);


    const dateStr = `${dayStr}/${monthStr}/${yearStr}`;

    const fileName = `GatezyVisitors_${dateStr}_${randomNum}.xlsx`;

    // Save the workbook
    workbook.xlsx.writeBuffer().then((data: any) => {
      const blob = new Blob([data], { type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' });
      FileSaver.saveAs(blob, fileName);
    });
  }




  onSearch() {
    //this.isLoading = true;
    if (this.startDate && this.endDate) {
      const start = new Date(this.startDate.year, this.startDate.month - 1, this.startDate.day);
      const end = new Date(this.endDate.year, this.endDate.month - 1, this.endDate.day);

      // Ensure 'end' date includes the entire day
      end.setHours(23, 59, 59, 999);

      // Filter the data based on check-in date
      this.filteredData = this.data.filter((item: any) => {
        if (!item.checkIn) return false;

        const checkInDate = new Date(item.checkIn);
        return checkInDate >= start && checkInDate <= end;
      });
    } else {
      // If no date range is selected, show all data
      this.filteredData = [...this.data];
    }

    setTimeout(() => {
      this.table = $('#dataGrid').DataTable({
        pagingType: 'simple_numbers',
        pageLength: 6,
        processing: true,
        destroy: true,
        lengthMenu: [10, 15, 20],
        searching: false,
        "dom": '<"top"i>rt<"bottom-table-option"flp><"clear">'
      });
    }, 100);

    this.isLoading = false;
  }

  openImageDialog(picture: string): void {
    this.dialog.open(ImageDialogComponent, {
      data: picture,
      panelClass: 'custom-dialog-content'
    });
  }
  forceCheckout(visitorId: string, mobileNumber: string): void {

    this._visitor.forceCheckout(visitorId, mobileNumber).subscribe({
      next: (response: any) => {
        if (response.status.message) {
          this.toastr.success('Visitor checked out successfully');

          this.getVisitors();

        } else {
          this.toastr.error(response.status.message, 'Error');
        }
      },

      error: (err: any) => {
        this.toastr.error('Failed to force check out the visitor', 'Error');
      }
    });
  }

}


