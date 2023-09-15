/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { UploadContractComponent } from './upload-contract.component';

describe('UploadContractComponent', () => {
  let component: UploadContractComponent;
  let fixture: ComponentFixture<UploadContractComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ UploadContractComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(UploadContractComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
