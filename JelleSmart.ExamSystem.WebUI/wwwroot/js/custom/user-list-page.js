"use strict";
var UserTable = function () {
	var selectedUserId = "";
	var initTable = function () {
		var table = $('#user-list-table');

		// begin first table
		table.DataTable({
			responsive: true,
			columnDefs: [				
				{
					width: '75px',
					targets: 4,
					render: function (data, type, full, meta) {
						var status = {
							1: { 'title': 'Aktif', 'class': 'label-light-success' },
							2: { 'title': 'Pasif', 'class': ' label-light-warning' },
							3: { 'title': 'Silinmiş', 'class': ' label-light-danger' },
							4: { 'title': 'Bloke Edilmiş', 'class': ' label-light-warning' }
						};
						if (typeof status[ data ] === 'undefined') {
							return data;
						}

						return '<span class="label label-lg font-weight-bold ' + status[ data ].class + ' label-inline">' + status[ data ].title + '</span>';
					}
				}
			]
		});

	};

	var addVacation = function () {
		$('#add_vacation_amount').TouchSpin({
			buttondown_class: 'btn btn-secondary',
			buttonup_class: 'btn btn-secondary',

			min: 0,
			max: 100,
			step: 1,
			boostat: 5,
			booster: true
			
		});
		$(".add-vacation").click(function () {
			var userId = $(this).data("id");
			selectedUserId = userId;
			$("#add-vacation-to-user-modal").modal();
		});
		$("#add_vacation_bt").click(function () {
			var model = {
				Id:selectedUserId,
				Amount: $("#add_vacation_amount").val(),
				Description: $("#add_vacation_description").val()
			}
			$.post("/UserAccount/AddUserVacation/", { model: model }).done(function (result) {
                if (result.isSuccess) {
					setTimeout(function () {
						window.location.reload();
					}, 3000);
					toastr.success("Başarılı", result.message);
                } else {
					toastr.error(result.message, "Bir Hata Oluştu");
                }
			});
		});
    };

	return {

		//main function to initiate the module
		init: function () {
			initTable();
			addVacation();
		}

	};

}();

jQuery(document).ready(function () {
	UserTable.init();
});
