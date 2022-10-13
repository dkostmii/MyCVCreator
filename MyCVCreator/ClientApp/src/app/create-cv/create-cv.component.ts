import { Component, OnInit, Inject } from '@angular/core';
import { Section } from '../model/section';
import { CV, Container } from '../model/cv';
import { HttpClient, HttpHeaders } from '@angular/common/http';

@Component({
  selector: 'app-create-cv',
  templateUrl: './create-cv.component.html',
  styleUrls: ['./create-cv.component.css']
})
export class CreateCvComponent implements OnInit {

  firstName: string = '';
  lastName: string = '';
  position: string = '';

  mainContainerTitle = 'Main content';
  sideContainerTitle = 'Sidebar';

  cv: CV;

  mainSections: Section[] = [];
  sideSections: Section[] = [];

  private http: HttpClient;
  private baseUrl: string;
  private apiUrl: string;

  constructor(
    http: HttpClient,
    @Inject('BASE_URL') baseUrl: string,
    @Inject('API_V1') apiUrl: string) {
    this.cv = {
      firstName: '',
      lastName: '',
      position: '',
      containers: [
        new Container('Sidebar'),
        new Container('Main content'),
      ]
    }

    this.http = http;
    this.baseUrl = baseUrl;
    this.apiUrl = apiUrl;
  }

  ngOnInit(): void { }

  private download(data: Blob, fileName: string) {
    const url = window.URL.createObjectURL(data);

    const link = document.createElement('a');
    link.setAttribute('href', url);
    link.setAttribute('download', fileName);
    link.style.display = 'none';
    document.body.appendChild(link);
    link.click();
    document.body.removeChild(link);
  }

  saveCV(): void {
    const url = this.baseUrl + this.apiUrl + 'cv';

    const headers = new HttpHeaders({
      "Accept": ['text/html']
    });

    this.http
      .post(url, this.cv, { headers, observe: 'response', responseType: 'blob' })
      .subscribe(res => {
        if (!res.ok) {
          console.error(`Unable to save CV. Status: ${res.statusText}`);
        }

        const cd = res.headers.get('Content-Disposition');
        const blob = res.body;

        if (cd === null || blob === null) {
          throw new Error('Unable to save CV. Missing file data.');
        }

        const fileName = cd
          .split('; ')
          .filter(t => t.includes("filename="))
          .map(t => t.replace("filename=", ""))[0];

        this.download(blob, fileName);
      });
  }
}
