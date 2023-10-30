import { ComponentFixture, TestBed } from '@angular/core/testing';

import { ContractTemplateComponent } from './contract-template.component';

describe('ContractTemplateComponent', () => {
  let component: ContractTemplateComponent;
  let fixture: ComponentFixture<ContractTemplateComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ ContractTemplateComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(ContractTemplateComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
