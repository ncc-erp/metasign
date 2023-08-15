import { TestBed } from '@angular/core/testing';

import { SignServerService } from './sign-server.service';

describe('SignServerService', () => {
  let service: SignServerService;

  beforeEach(() => {
    TestBed.configureTestingModule({});
    service = TestBed.inject(SignServerService);
  });

  it('should be created', () => {
    expect(service).toBeTruthy();
  });
});
