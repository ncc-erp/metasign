import { Component, Inject, OnInit } from '@angular/core';
import { ContractTemplateType } from '@app/module/contract-templates/enum/contract-template.enum';
import { ContractTemplateService } from '@app/service/api/contract-template.service';
import { CreateTemplateDto } from '../contract-template.component';
import { ContractService } from '@app/service/api/contract.service';
import * as pdfjsLib from "pdfjs-dist/webpack";
import { MAT_DIALOG_DATA, MatDialogRef } from '@angular/material/dialog';
import { ContractTemplates } from '@app/module/contract-templates/interface/contract-templates';

@Component({
  selector: 'app-upload-template',
  templateUrl: './upload-template.component.html',
  styleUrls: ['./upload-template.component.css']
})
export class UploadTemplateComponent implements OnInit {

  newTemplate = {} as any
  file: File;
  isConverting: boolean = false
  isUploadComplete: boolean = false
  title: string = ""

  constructor(private contractTemplateService: ContractTemplateService,
    private ref: MatDialogRef<UploadTemplateComponent>,
    @Inject(MAT_DIALOG_DATA) public data: ContractTemplates,
    private contractService: ContractService) { }

  ngOnInit(): void {
    if (this.data) {
      this.newTemplate = this.data
      this.title = `Update tempalte`
    }
    else{
      this.title = `Add template`
    }
  }
  save() {
    this.newTemplate.type = ContractTemplateType.System
    this.newTemplate.massType = 1

    if (!this.data) {
      this.contractTemplateService.createFileTemplate(this.newTemplate).subscribe(rs => {
        abp.notify.success("Created new template")
        this.ref.close(true)
      })
    }
    else {
      this.contractTemplateService.updateFileTemplate(this.newTemplate).subscribe(rs => {
        abp.notify.success("Created new template")
        this.ref.close(true)
      })
    }
  }

  onFileSelected(event): void {
    this.file = event.target.files[0];
    if (this.checkFileUpload(this.file?.type)) {
      this.isConverting = true;
      this.isUploadComplete = false;
      this.contractService.ConvertFile(this.file).subscribe(
        rs => {
          this.isUploadComplete = true;
          this.isConverting = false;
          this.newTemplate.content = rs.result.base64String
          this.newTemplate.fileName = rs.result.fileName
        },
        () => {
          this.isConverting = false;
          this.isUploadComplete = true;
          abp.message.error("convert file không thành công, hãy thử lại!")
        }
      );
      return;
    }

    const reader = new FileReader();
    reader.readAsDataURL(this.file);
    reader.onload = () => {
      this.isUploadComplete = true;

      this.newTemplate.content = reader.result.toString()
      this.newTemplate.fileName = event.target.files[0].name

    };
  };
  checkFileUpload(type: string) {
    const validImageTypes = ["application/pdf"];
    if (!validImageTypes.includes(type)) {
      return true;
    }
    return false;
  }

}
