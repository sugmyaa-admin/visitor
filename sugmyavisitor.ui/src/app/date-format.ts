import { NgbDateParserFormatter, NgbDateStruct } from '@ng-bootstrap/ng-bootstrap';
import { Injectable } from '@angular/core';

var months: any = ["Jan", "Feb", "Mar", "Apr", "May", "Jun", "Jul", "Aug", "Sep", "Oct", "Nov", "Dec"]
function padNumber(value: any | null) {
    if (!isNaN(value) && value !== null) {
        return `0${value}`.slice(-2);
    } else {
        return '';
    }
}
@Injectable()
export class NgbDateCustomParserFormatter extends NgbDateParserFormatter {
    parse(value: string): NgbDateStruct | null {
        if (value) {
            const dateParts = value.trim().split('/');

            let dateObj: any = { day: <any>null, month: <any>null, year: <any>null }
            const dateLabels = Object.keys(dateObj);

            dateParts.forEach((datePart: any, idx: any) => {
                dateObj[dateLabels[idx]] = parseInt(datePart, 10) || <any>null;
            });
            return dateObj;
        }
        return null;
    }

    format(date: NgbDateStruct | null): string {
        return date ?
            `${String(date.day).padStart(2, '0')}-${months[date.month-1]}-${date.year || ''}` :
            '';
    }
}