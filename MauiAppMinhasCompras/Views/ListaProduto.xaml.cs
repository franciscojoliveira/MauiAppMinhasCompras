using MauiAppMinhasCompras.Models;
using System.Collections.ObjectModel;

namespace MauiAppMinhasCompras.Views;

public partial class ListaProduto : ContentPage
{
	ObservableCollection<Produto> lista = new ObservableCollection<Produto>();
    private object categoria;

    public ListaProduto()
	{
		InitializeComponent();

		lst_produtos.ItemsSource = lista;
	}

    protected async override void OnAppearing()
    {
		try
		{
        
        lista.Clear();

		List<Produto> tmp = await App.Db.GetAll();

		tmp.ForEach( i => lista.Add(i));
        }
        catch (Exception ex)
        {
			await DisplayAlert("Ops", ex.Message, "OK");
        }

    }
    private void ToolbarItem_Clicked(object sender, EventArgs e)
    {
		try
		{
			Navigation.PushAsync(new Views.NovoProduto());

		} catch (Exception ex)
		{
			DisplayAlert("ops", ex.Message, "Ok");
		}

    }

    private async void txt_search_TextChanged(object sender, TextChangedEventArgs e)
    {
		try
		{
		
		string q = e.NewTextValue;

            lst_produtos.IsRefreshing = true;

            lista.Clear();

        List<Produto> tmp = await App.Db.Search(q);

        tmp.ForEach(i => lista.Add(i));
        }
        catch (Exception ex)
        {
			await DisplayAlert("Ops", ex.Message, "OK");
        }
        finally
        {
            lst_produtos.IsRefreshing = false;
        }
    }

    private void ToolbarItem_Clicked_1(object sender, EventArgs e)
    {
		double soma = lista.Sum(i => i.Total);

		string msg = $"O total é {soma:C}";

		DisplayAlert("Total dos Produtos", msg, "OK");
    }

    private async void MenuItem_Clicked(object sender, EventArgs e)
    {
        try
        {
            MenuItem selecionado = sender as MenuItem;

            Produto p = selecionado.BindingContext as Produto;

            bool confirm = await DisplayAlert(
                "Tem Certeza?", $"Remover {p.Descricao}?", "Sim", "Não");

            if (confirm)
            {
                await App.Db.Delete(p.Id);
                lista.Remove(p);   
            }

        }
        catch (Exception ex)
        {
           await DisplayAlert("Ops", ex.Message, "OK");
        }

    }

    private void lst_produtos_ItemSelected(object sender, SelectedItemChangedEventArgs e)
    {
        try
        {
            Produto p = e.SelectedItem as Produto;

            Navigation.PushAsync(new Views.EditarProduto
            {
                BindingContext = p,
            });
        }
        catch (Exception ex)
        {
           DisplayAlert("Ops", ex.Message, "OK");
        }
    }

    private async void lst_produtos_Refreshing(object sender, EventArgs e)
    {
        try
        {

            lista.Clear();

            List<Produto> tmp = await App.Db.GetAll();

            tmp.ForEach(i => lista.Add(i));
        }
        catch (Exception ex)
        {
            await DisplayAlert("Ops", ex.Message, "OK");
        } finally 
        {
            lst_produtos.IsRefreshing = false;
        }
    }

    private async void ToolbarItem_Clicked_2(object sender, EventArgs e)
    {

        {
            // Agrupa os produtos pela categoria e calcula o total por grupo
            var totaisPorCategoria = lista
                .GroupBy(produto => produto.Categoria) // Agrupa pela propriedade Categoria
                .Select(grupo => new
                {
                    Categoria = grupo.Key,               // A categoria
                    Total = grupo.Sum(produto => produto.Total) // Soma dos valores de 'Total' de cada produto
                });

            // Gera uma mensagem para exibir os totais por categoria
            string msg = string.Join("\n", totaisPorCategoria
                .Select(grupo => $"Categoria: {grupo.Categoria}, Total: {grupo.Total:C}"));

            // Exibe o resultado em uma mensagem
            DisplayAlert("Totais por Categoria", msg, "OK");
        }

       

    }
}