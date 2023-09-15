/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { DesignContractComponent } from './design-contract.component';

describe('DesignContractComponent', () => {
  let component: DesignContractComponent;
  let fixture: ComponentFixture<DesignContractComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ DesignContractComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(DesignContractComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
