import { ComponentFixture, TestBed } from '@angular/core/testing';

import { DetailContractComponent } from './detail-contract.component';

describe('DetailContractComponent', () => {
  let component: DetailContractComponent;
  let fixture: ComponentFixture<DetailContractComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ DetailContractComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(DetailContractComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
