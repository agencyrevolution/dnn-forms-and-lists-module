DNN Forms & Lists module
=====================================

Clone the source code from http://dnnfnl.codeplex.com/. It supports four views

- List

- Form

- Form Above List

- List Above Form

Each view contains two view templates

- DNN Grid view

- XSLT rendering

There is a requirement of Agency Rev designer team to enhance the current module to support Handlebars template.

----
Solution
----

- There are two rendering methods: Default Grid and XSLT Rendering. Add one more rendering method for Handlebars Template.

- If editor selects Handlebars Template rendering method

    + Show a button to let him know if he needs to create a new template or can edit the existing one.

    + Build a new HandlebarsTemplates server page to allow the editor edits the template, test it before saving to database.

**The default view**

If the view mode is List and rendering method is Handlebars Template, load a new server page to render the html content from Handlebars template

Just support basic search

- Keyword

- Pagination

```
<ul class="pages-pagination">

  {{#if pagination.previous.disabled}}
    <li class="disabled"><a href="#">&laquo;</a></li>
  {{else}}
    <li class="active"><a href="{{pagination.previous.url}}">&laquo;</a></li>
  {{/if}}

  {{#each pagination.pages}}
    {{#if active}}    
      <li class="active"><a href="{{url}}">{{pageIndex}} <span class="sr-only">(current)</span></a></li>
    {{else}}
      <li><a href="{{url}}">{{pageIndex}}</a></li>
    {{/if}}
  {{/each}}

  {{#if pagination.next.disabled}}
    <li class="disabled"><a href="#">&raquo;</a></li>
  {{else}}
    <li class="active"><a href="{{pagination.next.url}}">&raquo;</a></li>
  {{/if}}

</ul>
```