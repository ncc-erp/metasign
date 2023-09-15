import { Injector, Pipe, PipeTransform } from '@angular/core';
import { DatePipe } from '@angular/common';

@Pipe({
    name: 'formatDate'
})
export class FormatDatePipe implements PipeTransform {

    transform(date: Date | string) {
        return new DatePipe('en-US').transform(date, "dd/MM/yyyy");
    }
}

@Pipe({
    name: 'formatDateHour'
})
export class FormatDateHourPipe implements PipeTransform {
    transform(date: Date | string) {
        return new DatePipe('en-US').transform(date, "dd/MM/yyyy hh:mm:ss");
    }
}

@Pipe({
    name: 'formatDateHour24h'
})
export class FormatDateHour24hPipe implements PipeTransform {
    transform(date: Date | string) {
        return new DatePipe('en-US').transform(date, "dd/MM/yyyy | HH:mm:ss");
    }
}


@Pipe({
    name: 'formatDateHourSeconds'
})
export class FormatDateHourSecondsPipe implements PipeTransform {
    transform(date: Date | string) {
        return new DatePipe('en-US').transform(date, "dd/MM/yyyy HH:mm:ss");
    }
}