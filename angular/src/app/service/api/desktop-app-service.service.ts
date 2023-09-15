import { ApiResponseDto } from './../model/common.dto';
import { Injectable } from '@angular/core';
import { HttpClient } from '@angular/common/http';
import { BehaviorSubject, Observable } from 'rxjs';
@Injectable()

export class DesktopAppServiceService {
  url:string = "http://localhost:60371/api/Sign";
  $outputSignature:BehaviorSubject<any> = new BehaviorSubject(null)

  constructor(private httpClient: HttpClient) { }



  getCertificate(): Observable<any>
  {
      return this.httpClient.get<any>(this.url + `/GetCertInfor`)
  }

  SignDigital(input:any):Observable<string>{
      return this.httpClient.post<any>(this.url + `/SignMultiple`, input)
  }

}
