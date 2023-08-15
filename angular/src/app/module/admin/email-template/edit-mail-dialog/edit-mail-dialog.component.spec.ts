import { ComponentFixture, TestBed } from '@angular/core/testing';

import { EditMailDialogComponent } from './edit-mail-dialog.component';

describe('EditMailDialogComponent', () => {
  let component: EditMailDialogComponent;
  let fixture: ComponentFixture<EditMailDialogComponent>;

  beforeEach(async () => {
    await TestBed.configureTestingModule({
      declarations: [ EditMailDialogComponent ]
    })
    .compileComponents();
  });

  beforeEach(() => {
    fixture = TestBed.createComponent(EditMailDialogComponent);
    component = fixture.componentInstance;
    fixture.detectChanges();
  });

  it('should create', () => {
    expect(component).toBeTruthy();
  });
});
