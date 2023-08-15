import { MatDialogRef, MAT_DIALOG_DATA } from '@angular/material/dialog';
import { Component, Inject, Injector, OnInit } from '@angular/core';
import { AppComponentBase } from './app-component-base';

@Component({
    template: ''
})
export abstract class DialogComponentBase<TEntityDto> extends AppComponentBase {

    public dialogRef: MatDialogRef<any>;
    public dialogData: TEntityDto;
    public title: string = "";
    constructor(injector: Injector) {
        super(injector);
        this.dialogRef = injector.get(MatDialogRef)
        this.dialogData = injector.get(MAT_DIALOG_DATA)
    }
}
