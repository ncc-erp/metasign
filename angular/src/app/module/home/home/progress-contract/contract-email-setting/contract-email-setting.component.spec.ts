import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ContractEmailSettingComponent } from './contract-email-setting.component';

describe('ContractEmailSettingComponent', () => {
  let component: ContractEmailSettingComponent;
  let fixture: ComponentFixture<ContractEmailSettingComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ContractEmailSettingComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ContractEmailSettingComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
