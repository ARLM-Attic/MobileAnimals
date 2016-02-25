using System;
using Android.Widget;
using System.Collections.Generic;
using Android.App;
using Android.Views;
using System.Linq;
using MobileAnimal.Core;

namespace MobileAnimal.Droid
{
	public class AnimalAdapter : BaseAdapter<Animal>
	{
		private List<Animal> _items = new List<Animal>();
		private Activity _context;

		public AnimalAdapter(Activity context)
			: base()
		{
			_context = context;
		}

		public override long GetItemId(int position)
		{
			return position;
		}

		public override Animal this [int position] {   
			get { return _items[position]; } 
		}

		public override int Count {
			get { return _items.Count; } 
		}

		public override View GetView(int position, View convertView, ViewGroup parent)
		{
			View view = convertView; 

			if (view == null) { 
				view = _context.LayoutInflater.Inflate(Android.Resource.Layout.SimpleListItem1, null);
			}

			view.FindViewById<TextView>(Android.Resource.Id.Text1).Text = _items[position].Name;
			return view;
		}

		public void Add(Animal item)
		{
			_items.Add(item);
			NotifyDataSetChanged();
		}

		public void Clear()
		{
			_items.Clear();
			NotifyDataSetChanged();
		}

		public void Remove(Animal item)
		{
			_items.Remove(item);
			NotifyDataSetChanged();
		}
	}
}

