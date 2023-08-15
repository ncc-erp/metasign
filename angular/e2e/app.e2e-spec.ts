import { EContractTemplatePage } from './app.po';

describe('EContract App', function() {
  let page: EContractTemplatePage;

  beforeEach(() => {
    page = new EContractTemplatePage();
  });

  it('should display message saying app works', () => {
    page.navigateTo();
    expect(page.getParagraphText()).toEqual('app works!');
  });
});
