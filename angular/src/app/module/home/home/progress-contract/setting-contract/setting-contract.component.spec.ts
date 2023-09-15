/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { SettingContractComponent } from './setting-contract.component';

describe('SettingContractComponent', () => {
  let component: SettingContractComponent;
  let fixture: ComponentFixture<SettingContractComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SettingContractComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SettingContractComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
