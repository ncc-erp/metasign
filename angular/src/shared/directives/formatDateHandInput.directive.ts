import { Directive, ElementRef, HostListener } from '@angular/core';
import { NgControl } from '@angular/forms';

@Directive({
    selector: '[formatDateHandInputDirective]'
})
export class FormatDateHandInputDirective {
    constructor(private el: ElementRef) { }

    @HostListener('keydown.enter')
    onKeyDown() {
        this.formatDate();
    }

    @HostListener('keydown.enter')

    private formatDate() {
        const inputValue: string = this.el.nativeElement.value;
        const trimmedValue: string = inputValue.trim();

        if (trimmedValue.length === 6 || trimmedValue.length === 8) {
            const day: string = trimmedValue.slice(0, 2);
            const month: string = trimmedValue.slice(2, 4);
            const year: string = trimmedValue.slice(4);

            const parsedDay: number = parseInt(day);
            const parsedMonth: number = parseInt(month);
            const parsedYear: number = parseInt(year);

            if (
                parsedDay >= 1 && parsedDay <= 31 &&
                parsedMonth >= 1 && parsedMonth <= 12 &&
                parsedYear >= 0 && parsedYear <= 99
            ) {
                const fullYear: number = parsedYear < 50 ? 2000 + parsedYear : 1900 + parsedYear;
                const formattedDate: string = `${day}/${month}/${fullYear}`;
                this.el.nativeElement.value = formattedDate;

                // this.ngControl.control.setErrors(null);
            } else {
                // this.ngControl.control.setErrors({ 'invalidDate': true });
            }
        } else {
            // this.ngControl.control.setErrors({ 'invalidLength': true });
        }
    }
}
