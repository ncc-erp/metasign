import { ComponentFixture, TestBed } from '@angular/core/testing';

import { CertDetailDialogComponent } from './cert-detail-dialog.component';

describe('CertDetailDialogComponent', () => {
  let component: CertDetailDialogComponent;
  let fixture: ComponentFixture<CertDetailDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ CertDetailDialogComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(CertDetailDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
