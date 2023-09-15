/* tslint:disable:no-unused-variable */
import { async, ComponentFixture, TestBed } from '@angular/core/testing';
import { By } from '@angular/platform-browser';
import { DebugElement } from '@angular/core';

import { SigningRejectComponent } from './signing-reject.component';

describe('SigningRejectComponent', () => {
  let component: SigningRejectComponent;
  let fixture: ComponentFixture<SigningRejectComponent>;

  beforeEach(async(() => {
    TestBed.configureTestingModule({
      declarations: [ SigningRejectComponent ]
    })
    .compileComponents();
  }));

  beforeEach(() => {
    fixture = TestBed.createComponent(SigningRejectComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
