import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ContractInputComponent } from './contract-input.component';

describe('ContractInputComponent', () => {
  let component: ContractInputComponent;
  let fixture: ComponentFixture<ContractInputComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ContractInputComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ContractInputComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
